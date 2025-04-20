import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Component, ViewChild, AfterViewInit, inject, Input} from '@angular/core';
import {MatPaginator, MatPaginatorModule} from '@angular/material/paginator';
import {MatSort, MatSortModule, SortDirection} from '@angular/material/sort';
import {merge, Observable, of as observableOf} from 'rxjs';
import {catchError, map, startWith, switchMap} from 'rxjs/operators';
import {MatTableModule} from '@angular/material/table';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {DatePipe} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MatGridListModule} from '@angular/material/grid-list';
import {MatButtonModule} from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
} from '@angular/material/dialog';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import { SensorDialog } from './SensorsDialog/SensorDialog.component';
import { FileEntity, FilesComponent } from '../Files/Files.component';
import { Equipment } from '../Equipment/Equipment.component';
@Component({
  selector: 'Sensors',
  styleUrl: 'Sensors.component.scss',
  templateUrl: 'Sensors.component.html',
  standalone: true,
  imports: [MatProgressSpinnerModule, MatTableModule, MatSortModule, MatPaginatorModule, DatePipe,
            MatFormFieldModule, MatInputModule, MatGridListModule, FormsModule, MatButtonModule, FilesComponent],
})
export class SensorsComponent implements AfterViewInit {
  @Input() equipmentId:number|null
  displayedColumns: string[] = ['title'];
  exampleDatabase: SensorsHttpDatabase | null;
  data: SensorEntity[] = new Array();
  readonly dialog = inject(MatDialog);
  isLoadingResults = true;
  initialized = false;
  isRateLimitReached = false;
  activeRow:SensorEntity | null | undefined = null;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private _httpClient: HttpClient) {}
  ngOnChanges()
  {
    if(this.initialized)
      {
        this.isLoadingResults = true;
        return this.exampleDatabase!.getSensorsRepo(
          this.equipmentId
        ).pipe(catchError(() => observableOf(null))).subscribe(data => {
          if(data !== null)
          {
            this.data = data.sensors;
            this.isLoadingResults = false;
            this.activeRow = this.data[0];
          }
          return;
        });
      }
      return;
  }
  ngAfterViewInit() {
    this.exampleDatabase = new SensorsHttpDatabase(this._httpClient);
    this.initialized = true;
    this.equipmentId = 1;
  }
  openDialog(): void {
    this.exampleDatabase = new SensorsHttpDatabase(this._httpClient);

    const dialogRef = this.dialog.open(SensorDialog, {
      data: {},
    });

    dialogRef.componentInstance._SensorsRepo = this.exampleDatabase;
    dialogRef.componentInstance.equipmentId = this.equipmentId;
    dialogRef.afterClosed().subscribe(result => {
      this.ngOnChanges();
    });
  }
}

export interface SensorEntity {
  id: number;
  title :number;
  date:number;
  files :FileEntity[]
}

export class SensorsHttpDatabase {

  private authSecretKey = 'Bearer Token';
  constructor(private _httpClient: HttpClient) {}
  private certKey = 'Cert';

  href = 'https://vibroslleznov.zapto.org:4445/equipment';
  private getHeaders(): HttpHeaders {
    const authToken = localStorage.getItem(this.authSecretKey);
    const certToken = localStorage.getItem(this.certKey);
    var httpHeaders = new HttpHeaders({
      Authorization: `Bearer ${authToken}`
    });
    httpHeaders = httpHeaders.set('CertificateKey',  `${certToken}`);
    return httpHeaders;
  }
  getSensorsRepo(equipmentId: number|null|undefined): Observable<Equipment> {
    const requestUrl = `${this.href}/${equipmentId}`;
    const headers = this.getHeaders();

    return this._httpClient.get<Equipment>(requestUrl, { headers });
  }
  addOrEditSensor(Sensor: FormData): Observable<SensorEntity> {

    const recRef = 'https://vibroslleznov.zapto.org:4445/sensors';

    const requestUrl = `${recRef}`;
    const headers = this.getHeaders();

    return this._httpClient.post<SensorEntity>(requestUrl, Sensor, { headers });
  }
}
