import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router, ActivatedRoute  } from '@angular/router';
import { FrequencyModel } from '../Enities/FrequencyModel'
import { Sensor } from '../Enities/Sensor';
import { SensorDataModel } from '../Enities/SensorDataModel';
import { TimeValueModel } from '../Enities/TimeValueModel';
import { PointChartComponent } from '../point-chart/point-chart.component';
import { AnaliticComponent } from '../analitic/analitic.component';
import { SimpleChartComponent } from '../simple-chart/simple-chart.component';
import { Location } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: 'sensor-analitic.component.html',
  styleUrls: ['sensor-analitic.component.scss'],
  standalone: true,
  imports: [AnaliticComponent, PointChartComponent, SimpleChartComponent]
})
export class SensorAnaliticComponent implements OnInit, AfterViewInit {
  public sensor: Sensor;
  public sensorData: SensorDataModel[];
  public data: FrequencyModel[];
  public dataAlg: TimeValueModel[];
  private currentValue:FrequencyModel[];
  constructor( 
              private router: Router, 
              private location: Location,
              private route: ActivatedRoute) 
  {
    try {
      var a = location.getState() as {sensorState: string};
      var b = a.sensorState;
      var n = b.lastIndexOf(']');
      var result = b.substring(0,n+1);
      let obj = JSON.parse(result);
      this.sensorData = obj
    }
    catch (error) {
      let msg = (error as Error).message;
    }
    this.sensor = new Sensor();
    this.sensor.averageValues = [{date : new Date("2019-01-16"), value : 1},{date : new Date("2019-01-17"), value : 2}]
    this.sensor.sensorValues = this.sensorData
    this.data = []
    this.dataAlg = []
    this.data = this.sensorData[0].SensorsFrequencyDataPoints.filter(x => x.Power < 100)
  }
  displayedColumns: string[] = ['id', 'name'];

  private updateData(value:FrequencyModel[])
  {
    this.currentValue = value
  }
  ngOnInit() {
  }
  ngAfterViewInit() {

  }
}