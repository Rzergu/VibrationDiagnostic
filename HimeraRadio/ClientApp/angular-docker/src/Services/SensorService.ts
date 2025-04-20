import { Injectable,OnInit } from '@angular/core';
import { Sensor } from '../app/Enities/Sensor';
import { EventEmitter } from '@angular/core'
import { FrequencyModel } from '../app/Enities/FrequencyModel';

import {merge, Observable, of as observableOf} from 'rxjs';

@Injectable({ providedIn: 'root',})
export class SensorsService {
  private sensorsMap = new Map<number, EventEmitter<FrequencyModel[]>>()
  constructor()
  {
    this.sensorsMap = new Map<number, EventEmitter<FrequencyModel[]>>()
    this.sensorsMap.set(1, new EventEmitter<FrequencyModel[]>())
  }
  public RegisterSensor(id: number)
  {
    this.sensorsMap.set(id, new EventEmitter<FrequencyModel[]>())
  }
  public SubscribeToSensor(id: any, callback: any)
  {
    this.sensorsMap.get(1)?.subscribe(callback)
  }
  public SetCurrentValueSensor(id: number, value: FrequencyModel[] | undefined)
  {
    this.sensorsMap.get(id)?.emit(value)
  }
}
