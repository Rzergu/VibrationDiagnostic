import {HttpClient} from '@angular/common/http';
import {Component, ViewChild, AfterViewInit} from '@angular/core';
import {DatePipe} from '@angular/common';
import { SensorsComponent } from '../Components/Sensors/Sensors.component';
import { FilesComponent } from '../Components/Files/Files.component';
import { EquipmentsComponent } from "../Components/Equipment/Equipment.component";
import { Router } from '@angular/router';

import { HttpClientModule } from '@angular/common/http';
import { RouterOutlet} from "@angular/router";

@Component({
  selector: 'app-root',
  styleUrl: 'app.component.scss',
  templateUrl: 'app.component.html',
  standalone: true,
  imports: [SensorsComponent, FilesComponent, DatePipe, EquipmentsComponent, RouterOutlet, HttpClientModule],
})
export class AppComponent implements AfterViewInit {

  constructor(private router : Router, private _httpClient: HttpClient) {}

  ngAfterViewInit() {
  }
}
