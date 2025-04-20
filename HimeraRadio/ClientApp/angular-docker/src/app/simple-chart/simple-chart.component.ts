import { HostListener, Directive,SimpleChanges, Component, OnInit, Input, OnChanges, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FrequencyModel } from '../Enities/FrequencyModel'
import { SensorsService } from '../../Services/SensorService';
import * as d3 from 'd3';

@Component({
  selector: 'simple-chart',
  templateUrl: 'simple-chart.component.html',
  styleUrls: ['simple-chart.component.scss'],
  standalone: true
})
export class SimpleChartComponent implements OnInit {
  @Input() transitionTime = 1;
  @Input() hticks = 60;
  @Input() data: FrequencyModel[];
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
  x:any; // X-axis graphical coordinates
  y:any; // Y-axis graphical coordinates
  colors = d3.scaleOrdinal(d3.schemeCategory10);
  bins:any; // Array of frequency distributions - one for each area chaer
  paths:any; // Path elements for each area chart
  areaGenerator:any;
  area:any; // For D3 area function
  histogram:any; // For D3 histogram function

  constructor(private elRef: ElementRef, public sensorService: SensorsService) {
      this.sensorService = sensorService;
      this.hostElement = this.elRef.nativeElement;
      this.data = this.data;  
    }

  private updateData(value:FrequencyModel[])
  {
    this.data = value
    this.updateChart()
  }

  ngOnInit() {
    this.updateChart()
  }
  ngAfterViewInit() {
      this.sensorService.SubscribeToSensor(1,this.updateData.bind(this))
  }

  private createChart() {

      this.removeExistingChartFromParent();

      this.setChartDimensions();

      this.addGraphicsElement();

      this.createXAxis();

      this.createYAxis();

      // d3 area and histogram functions  has to be declared after x and y functions are defined
      this.areaGenerator = d3.area()
          .x((datum: any) => this.x(datum.Frequency))
          .y0(this.y(0))
          .y1((datum: any) => this.y(datum.Power));

      this.createAreaCharts();
      this.setBrushTool();

  }
  private idled() { this.idleTimeout = null; }
  private setBrushTool(){
    this.clip = this.svg.append("defs").append("svg:clipPath")
    .attr("id", "clip")
    .append("svg:rect")
    .attr('transform', 'translate(30,0)')
    .attr("width", 580 )
    .attr("height", 280 )
    this.area = this.svg.append('g')
    .attr("clip-path", "url(#clip)")
    this.brush = d3.brushX()                   // Add the brush feature using the d3.brush function
    .extent( [ [0,0], [580,180] ] )  // initialise the brush area: start at 0,0 and finishes at width,height: it means I select the whole graph area
    .on("end", this.zoomChart.bind(this))
    this.area
    .append("g")
      .attr("class", "brush")
      .call(this.brush);
    
    this.svg.on("dblclick",() =>{
        this.x.domain(d3.extent(this.data, function(d:any) { return d.frequency; }))
        this.xAxis.transition().call(d3.axisBottom(this.x))
        this.area
            .select('.myArea')
            .transition()
            .attr("d", this.areaGenerator(this.data))
        });
  }
  private setChartDimensions() {
      let viewBoxHeight = 300;
      let viewBoxWidth = 600;
      this.svg = d3.select(this.hostElement).append('svg')
          .attr('width', '100%')
          .attr('height', '165%')
          .attr('viewBox', '0 0 ' + viewBoxWidth + ' ' + viewBoxHeight);
  }

  private addGraphicsElement() {
      this.g = this.svg.append("g")
          .attr("transform", "translate(0,0)");
  }


  private createXAxis() {
      this.x = d3.scaleLinear()
          .domain([0, Math.max(...this.data.map(item => item.Frequency ?? 1000))])
          .range([30, 580]);

      this.xAxis =this.g.append('g')
          .attr('transform', 'translate(0,150)')
          .attr("stroke-width", 0.5)

      this.xAxis.call(d3.axisBottom(this.x).tickSize(0).tickFormat(<any>''));

      this.xAxisNet = this.g.append('g')
          .attr('transform', 'translate(0,150)')
          .style('font-size', '6')
          .style("stroke-dasharray", ("1,1"))
          .attr("stroke-width", 0.1)

      this.xAxisNet.call(d3.axisBottom(this.x).ticks(10).tickSize(-140));

  }

  private createYAxis() {
      this.y = d3.scaleLinear()
          .domain([0, Math.max(...this.data.map(item => item.Power ?? 10))])
          .range([150, 10]);

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
          .attr('fill', 'white')
          .attr('transform', 'translate(10,80) rotate(-90)')
          .style('font-size', 12)
          .text('Мощность');
      }
  }
  private createAreaCharts() {
    this.paths = [];
    this.paths.push(this.g.append('path')
    .attr('fill', "steelblue")
    .attr("stroke-width", 3)
    .attr('opacity', 0.9)
    .attr("clip-path", "url(#clip)")
    .attr('d', this.areaGenerator(this.data))
    );

  }
  private zoomChart(event: { selection: any; }) {
      this.extent = event.selection

      if(this.extent){
        this.x = d3.scaleLinear().domain([ this.x.invert(this.extent[0]), this.x.invert(this.extent[1]) ]).range([30, 580])
        this.area.select(".brush").call(this.brush.move, null)
      }
      this.xAxis.transition().duration(this.transitionTime).call(d3.axisBottom(this.x).tickSize(0).tickFormat(<any>''));
      this.xAxisNet.transition().duration(this.transitionTime).call(d3.axisBottom(this.x).ticks(10).tickSize(-160));

      this.updateAreaCharts();
  }
  public updateChart() {
      if (!this.svg) {
          this.createChart();
          return;
      }

      this.x = d3.scaleLinear()
      .domain([0, Math.max(...this.data.map(item => item.Frequency ?? 1000))])
      .range([30, 580]);
      this.xAxis.transition().duration(this.transitionTime).call(d3.axisBottom(this.x).tickSize(0).tickFormat(<any>''));
      this.xAxisNet.transition().duration(this.transitionTime).call(d3.axisBottom(this.x));

      this.y = d3.scaleLinear()
      .domain([0, Math.max(...this.data.map(item => item.Power ?? 10))])
      .range([150, 10]);
      this.yAxis.transition().duration(this.transitionTime).call(d3.axisLeft(this.y).tickSize(0).tickFormat(<any>''));
      this.yAxisNet.transition().duration(this.transitionTime).call(d3.axisLeft(this.y))
      .style('font-size', '6');

      this.updateAreaCharts();
  }

  private updateAreaCharts() {
      this.paths.forEach((path:any) => {
          path.datum(this.data)
              .transition().duration(this.transitionTime)
              .attr("clip-path", "url(#clip)")
              .attr('d', d3.area()
                  .x((datum: any) => this.x(datum.frequency))
                  .y0(this.y(0))
                  .y1((datum: any) => this.y(datum.power)));
      });
  }

  private removeExistingChartFromParent() {
      d3.select(this.hostElement).select('svg').remove();
  }
}
