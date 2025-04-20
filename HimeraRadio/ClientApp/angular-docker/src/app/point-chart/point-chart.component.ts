import { HostListener, Directive,SimpleChanges, Component, OnInit, Input, OnChanges, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { TimeValueModel } from '../Enities/TimeValueModel'
import {formatDate} from '@angular/common';
import * as d3 from 'd3';
import { Sensor } from '../Enities/Sensor';

@Component({
  selector: 'point-chart',
  templateUrl: 'point-chart.component.html',
  styleUrls: ['point-chart.component.scss'],
  standalone: true
})
export class PointChartComponent implements OnInit,OnChanges {
  @Input() transitionTime = 1000;
  @Input() hticks = 60;
  @Input() data: Sensor;
  @Input() sensorId: number;
  @Input() showLabel = 1;
  hostElement; // Native element hosting the SVG container
  svg:any; // Top level SVG element
  g:any; // SVG Group element
  colorScale:any; // D3 color provider
  xAxis:any;
  yAxis:any;
  brush:any;
  xAxisNet:any;
  idleTimeout:any;
  extent:any;
  clip:any;
  yAxisNet:any;
  areaGenerator:any;
  x:any; // X-axis graphical coordinates
  y:any; // Y-axis graphical coordinates
  colors = d3.scaleOrdinal(d3.schemeCategory10);
  bins:any; // Array of frequency distributions - one for each area chaer
  paths:any; // Path elements for each area chart
  area:any; // For D3 area function
  histogram:any; // For D3 histogram function

  constructor(private elRef: ElementRef) {
      this.hostElement = this.elRef.nativeElement;
      this.data = this.data;  
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['data'] && !changes['data'].firstChange) {
            this.data = changes['data'].currentValue
            this.updateChart();
        }
    }
  ngOnInit() {
    this.updateChart();
  }

  private createChart() {

      this.removeExistingChartFromParent();

      this.setChartDimensions();

      this.addGraphicsElement();

      this.createXAxis();

      this.createYAxis();

      this.areaGenerator = d3.area()
          .x((datum: any) => this.x(formatDate(datum.date, 'yyyy-MM-dd HH:mm:ss', 'en-US')))
          .y0(this.y(0))
          .y1((datum: any) => this.y(datum.value));
      this.createAreaCharts();
      this.setBrushTool();

  }
  private setBrushTool(){
    this.clip = this.svg.append("defs").append("svg:clipPath")
    .attr("id", "clip")
    .append("svg:rect")
    .attr('transform', 'translate(30,0)')
    .attr("width", 550 )
    .attr("height", 280 )
    this.area = this.svg.append('g')
    .attr("clip-path", "url(#clip)")
    this.brush = d3.brushX()                   // Add the brush feature using the d3.brush function
    .extent( [ [0,0], [580,280] ] )  // initialise the brush area: start at 0,0 and finishes at width,height: it means I select the whole graph area
    .on("end", this.zoomChart.bind(this))
    this.area
    .append("g")
      .attr("class", "brush")
      .call(this.brush);
    
    this.svg.on("dblclick",() =>{
        this.xAxis.transition().call(d3.axisBottom(this.x))
        this.area
            .select('.myArea')
            .transition()
            .attr("d", this.areaGenerator(this.data.averageValues))
        });
  }
  private setChartDimensions() {
      let viewBoxHeight = 300;
      let viewBoxWidth = 600;
      this.svg = d3.select(this.hostElement).append('svg')
          .attr('width', '100%')
          .attr('height', '100%')
          .attr('viewBox', '0 0 ' + viewBoxWidth + ' ' + viewBoxHeight);
  }

  private addGraphicsElement() {
      this.g = this.svg.append("g")
          .attr("transform", "translate(0,0)");
  }


  private createXAxis() {
      this.x = d3.scalePoint()
          .domain(this.data.averageValues.map(data => formatDate(data.date, 'yyyy-MM-dd HH:mm:ss', 'en-US')))
          .range([30, 580]);

      this.xAxis =this.g.append('g')
          .attr('transform', 'translate(0,270)')
          .attr("stroke-width", 0.5)

      this.xAxis.call(d3.axisBottom(this.x).tickSize(0).tickFormat(<any>''));

      this.xAxisNet = this.g.append('g')
          .attr('transform', 'translate(0,300)')
          .style('font-size', '6')
          .style("stroke-dasharray", ("1,1"))
          .attr("stroke-width", 0.9)

      //this.xAxisNet.call(d3.axisBottom(this.x).ticks(10).tickSize(-260));

  }

  private createYAxis() {
      this.y = d3.scaleLinear()
          .domain([0, Math.max()])
          .range([270, 10]);

      this.yAxis = this.g.append('g')
          .attr('transform', 'translate(30,0)')
          .attr("stroke-width", 0.5)

      this.yAxis.call(d3.axisLeft(this.y).tickSize(0).tickFormat(<any>''));
      
      this.yAxisNet = this.g.append('g')
          .attr('transform', 'translate(30,0)')
          .style("stroke-dasharray", ("1,1"))
          .attr("stroke-width", 0.1)

      this.yAxisNet.call(d3.axisLeft(this.y).ticks(4).tickSize(-550))
          .style('font-size', '6');

      if (this.showLabel === 1) {
          this.g.append('text')
          .attr('text-anchor', 'middle')
          .attr('transform', 'translate(10,135) rotate(-90)')
          .style('font-size', 14)
          .attr('fill', "white")
          .text('Отклонение');
      }
  }
  private createAreaCharts() {
    this.paths = [];
    this.paths.push(this.g.append('path')
    .attr('fill', "steelblue")
    .attr("stroke-width", 3)
    .attr('opacity', 0.9)
    .attr("clip-path", "url(#clip)")
    .attr('d', this.areaGenerator(this.data.averageValues))
    );

  }
  private zoomChart(event: { selection: any; }) {
      this.extent = event.selection

      if(this.extent){
        this.x = d3.scaleLinear().domain([ this.x.invert(this.extent[0]), this.x.invert(this.extent[1]) ]).range([30, 580])
        this.area.select(".brush").call(this.brush.move, null)
      }
      this.xAxis.transition().duration(this.transitionTime).call(d3.axisBottom(this.x).tickSize(0).tickFormat(<any>''));
      this.xAxisNet.transition().duration(this.transitionTime).call(d3.axisBottom(this.x).ticks(10).tickSize(-260));

      this.updateAreaCharts();
  }
  public updateChart() {
      if (!this.svg) {
          this.createChart();
          return;
      }

      this.x = d3.scalePoint()
        .domain(this.data.averageValues.map(data => formatDate(data.date, 'yyyy-MM-dd HH:mm:ss', 'en-US')))
        .range([30, 580]);
      this.xAxis.transition().duration(this.transitionTime).call(d3.axisBottom(this.x).tickSize(0).tickFormat(<any>''));
      //this.xAxisNet.transition().duration(this.transitionTime).call(d3.axisBottom(this.x).ticks(10).tickSize(-260));

      this.y = d3.scaleLinear()
      .domain([0, Math.max()])
      .range([270, 10]);
      this.yAxis.transition().duration(this.transitionTime).call(d3.axisLeft(this.y).tickSize(0).tickFormat(<any>''));
      this.yAxisNet.transition().duration(this.transitionTime).call(d3.axisLeft(this.y).ticks(4).tickSize(-550))
      .style('font-size', '6');

      this.updateAreaCharts();
  }

  private updateAreaCharts() {
      this.paths.forEach((path:any) => {
          path.datum(this.data.averageValues)
              .transition().duration(this.transitionTime)
              .attr("clip-path", "url(#clip)")
              .attr('d', d3.area()
                  .x((datum: any) => this.x(formatDate(datum.date, 'yyyy-MM-dd HH:mm:ss', 'en-US')))
                  .y0(this.y(0))
                  .y1((datum: any) => this.y(datum.value)));
      });
  }

  private removeExistingChartFromParent() {
      d3.select(this.hostElement).select('svg').remove();
  }
}
