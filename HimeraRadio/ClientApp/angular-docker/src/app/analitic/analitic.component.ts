import { HostListener, Directive,SimpleChanges, Component, OnInit, Input, OnChanges, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import * as d3 from 'd3';
import { Sensor } from '../Enities/Sensor';
import { SensorDataModel } from '../Enities/SensorDataModel';
import { SensorsService } from '../../Services/SensorService';
import { FrequencyModel } from '../Enities/FrequencyModel';
import {formatDate} from '@angular/common';
import * as THREE from "three";
import { OrbitControls } from "three/addons/controls/OrbitControls.js";

@Component({
  selector: 'analitic-graph',
  templateUrl: 'analitic.component.html',
  styleUrls: ['analitic.component.scss'],
  standalone: true
})
export class AnaliticComponent implements OnInit, OnChanges  {
  @Input() transitionTime = 1000;
  @Input() hticks = 60;
  @Input() data: SensorDataModel[];
  @Input() showLabel = 1;
    countryName: any;
    peaks: any;
    isDataPart: boolean;
    _sensorService: SensorsService;
    anchors: any[];
    anchorsProjection: (camera: any, projVectors: any, halfSize: any) => void;

    constructor(private elRef: ElementRef, public sensorService: SensorsService) {
      this._sensorService = sensorService
    }

    ngOnChanges(changes: SimpleChanges): void {

    }
    ngOnInit(): void {
        var waterfall = new WaterfallDiagram(this);
        waterfall.start(this._sensorService, this.data);
    }
}
var WaterfallDiagram = function(this: any){
    this.d3 = {
      camera: null,
      scene: null,
      renderer: null,
      lineMaterial: null,
      shapeMaterial: null
    }
  
  
    this.audio = {
      context: null,
      analyser: null,
      input: null,
      inputPoint: null,
      freqByteData: null,
    }
  
    this.curves = [];
    this.startTimestamp = null;
    this.frameCounter = 0;
  
    this.config = {
      maxCurves: 140,
      skipFrames: 0,
      movementSpeed: 1,
      fftSize: 1024,
      minFreqHz: 0,
      maxFreqHz: 6000
    }
  
    this.fftWindowSize = {
      from: null,
      to: null
    }
    const newVariable: any = window.navigator;
  
    newVariable.requestAnimationFrame = newVariable.requestAnimationFrame ||
    newVariable.webkitRequestAnimationFrame ||
    newVariable.mozRequestAnimationFrame;
  
    this.init3d();
  } as unknown as WaterfallConstructor;

  interface WaterfallConstructor {
    new(obj: any): IStartable;
    }
    interface IStartable {
        obj: object;
        start(service:SensorsService, data: SensorDataModel[]): void;
    }

  WaterfallDiagram.prototype.getNewData = function() {
    this.audio.analyser.getByteFrequencyData(this.audio.freqByteData);
    return this.audio.freqByteData.subarray(
      this.fftWindowSize.from, this.fftWindowSize.to);
  }
  
  WaterfallDiagram.prototype.makeShape = function(data: string | any[], width: number, height: number) {
    var shape = new THREE.Shape();
    var step = width / (data.length + 1);
    var len = data.length;
  
    shape.moveTo( 0, 0 );
    for(var i = 0; i < len; i++) {
      shape.lineTo(step * (i + 1), data[i] * height);
    }
    shape.lineTo( width, 0 );
  
    return shape;
  }
  
  WaterfallDiagram.prototype.makeObjects = function(shape: THREE.Shape) {
    var group = new THREE.Group();
  
    // flat shape
    var geometry = new THREE.ShapeGeometry( shape );
    var mesh = new THREE.Mesh( geometry, this.d3.shapeMaterial );
  
    // solid line
    var line = new THREE.Line( geometry, this.d3.lineMaterial );
  
    group.add( line );
    group.add( mesh );
    group.rotation.x = Math.PI * 0.5;
    return group;
  }
  
  WaterfallDiagram.prototype.init3d = function() {
    this.d3.scene = new THREE.Scene();
  
    this.d3.camera = new THREE.PerspectiveCamera(
      45, window.innerWidth / window.innerHeight, 1, 2000);
    this.d3.camera.rotation.x = Math.PI * 0.33;
    this.d3.camera.position.set( 150, -200, 230 );
    this.d3.scene.add( this.d3.camera );
  
    var light = new THREE.PointLight( 0xffffff, 0.8 );
    this.d3.camera.add( light );
  
    this.d3.lineMaterial=  new THREE.LineBasicMaterial({
      color: 0x000000, linewidth: 1
    });
    this.d3.shapeMaterial=  new THREE.MeshBasicMaterial({
      color: 0xffffff, side: THREE.DoubleSide
    });
    this.container = document.getElementById( 'canvas' );
    this.d3.renderer = new THREE.WebGLRenderer( { antialias: true } );
    this.d3.renderer.setClearColor( 0xffffff );
    this.d3.renderer.setPixelRatio( window.devicePixelRatio );
    this.d3.renderer.setSize( this.container?.clientWidth, this.container?.clientHeight);
    this.container?.appendChild( this.d3.renderer.domElement );
  }
  
  WaterfallDiagram.prototype.resizeWindow = function() {
    this.d3.renderer.setSize(this.container?.clientWidth, this.container?.clientHeight);
  }
  
  WaterfallDiagram.prototype.addNewData = function(sensorData: SensorDataModel) {
    // only every n-th frame a new data set is inserte
  
    // get new dataset
    //var data = this.getNewData();
    // get new dataset
    var data = sensorData.SensorsFrequencyDataPoints.slice(2).map ((x:any) => x.Power).filter(x => x < 500);
    var frq = 0
    var dataArray: FrequencyModel[] = []
    sensorData.SensorsFrequencyDataPoints.forEach((x: any) => {
      var res = new FrequencyModel(x.Frequency, x.Power);
      dataArray.push(res)
    });
    this.sensorService.SetCurrentValueSensor(1,dataArray)
    var shape = this.makeShape(data, 300, 3);
    var curve = this.makeObjects(shape);
    this.curves.unshift(curve);
    this.d3.scene.add(curve);
  
    // insert it and remove old
    if(this.curves.length > this.config.maxCurves) {
      this.d3.scene.remove(this.curves.pop());
    }
    this.render();
  }
  
  WaterfallDiagram.prototype.render = function(timestamp: number) {
    if (!this.startTimestamp) {
      this.startTimestamp = timestamp;
    }
    var progress = timestamp - this.startTimestamp;
  
    var movementSpeed = this.config.movementSpeed;
    this.curves.forEach(function(curve: { position: { y: any; }; }) {
      curve.position.y += movementSpeed;
    });
  
    this.d3.renderer.render(this.d3.scene, this.d3.camera);
  }
  
  WaterfallDiagram.prototype.gotStream = function(stream: any) {
      this.audio.inputPoint = this.audio.context.createGain();
  
      // Create an AudioNode from the stream.
      this.audio.input = this.audio.context.createMediaStreamSource(stream);
      this.audio.input.connect(this.audio.inputPoint);
  
      this.audio.analyser = this.audio.context.createAnalyser();
      this.audio.analyser.fftSize = this.config.fftSize;
      this.audio.freqByteData = new Uint8Array(this.audio.analyser.frequencyBinCount);
      this.audio.inputPoint.connect( this.audio.analyser );
  }
  
  WaterfallDiagram.prototype.calcFFTWindowSize = function() {
    var maxFreq = this.audio.context.sampleRate / 2;
    var sizePerFreq = this.config.fftSize / maxFreq;
    this.fftWindowSize.from = Math.floor(sizePerFreq * this.config.minFreqHz);
    this.fftWindowSize.to = Math.floor(sizePerFreq * this.config.maxFreqHz);
  }
  
  WaterfallDiagram.prototype.initAudio = function() {
    window.AudioContext  = window.AudioContext || window.AudioContext;
  
    this.audio.context = new window.AudioContext();
    this.calcFFTWindowSize();
    const newVariable: any = window.navigator;
    newVariable.getUserMedia = newVariable.getUserMedia ||
    newVariable.webkitGetUserMedia ||
    newVariable.mozGetUserMedia;
  
    if (newVariable.getUserMedia) {
        newVariable.getUserMedia(
        {audio: true},
        this.gotStream.bind(this),
        function(e: any) {
          alert('Error getting audio');
          console.log(e);
      });
    } else {
      alert('Error getting audio');
    }
  }
  
  WaterfallDiagram.prototype.start = function(service:SensorsService, dataS: SensorDataModel[]) {
    //this.initAudio();
    //requestAnimationFrame( this.addNewData.bind(this) );
    //requestAnimationFrame( this.render.bind(this) );
    this.sensorService = service;
    this.sensorData = dataS;
    this.sensorData.sort((a:SensorDataModel,b:SensorDataModel)=>a.date > b.date).forEach((element: SensorDataModel) => {
        this.addNewData(element)
    });
  }