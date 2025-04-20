import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Component, ViewChild, AfterViewInit, inject} from '@angular/core';
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
import { SensorEntity, SensorsComponent } from '../Sensors/Sensors.component';
import { EquipmentDialog } from './EquipmentDialog/EquipmentDialog.component';
@Component({
  selector: 'equipments',
  styleUrl: 'Equipment.component.scss',
  templateUrl: 'Equipment.component.html',
  standalone: true,
  imports: [MatProgressSpinnerModule, MatTableModule, MatSortModule, MatPaginatorModule, DatePipe,
    MatFormFieldModule, MatInputModule, MatGridListModule, FormsModule, MatButtonModule, SensorsComponent],
})
export class EquipmentsComponent implements AfterViewInit {
  displayedColumns: string[] = ['name'];
  exampleDatabase: EquipmentsHttpDatabase | null;
  data: Equipment[] = [];
  readonly dialog = inject(MatDialog);
  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;
  activeRow:Equipment | null | undefined= null;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private _httpClient: HttpClient) {}

  ngAfterViewInit() {
    this.exampleDatabase = new EquipmentsHttpDatabase(this._httpClient);

    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          return this.exampleDatabase!.getEquipmentsSensors(
            this.sort.active,
            this.sort.direction,
            this.paginator.pageIndex,
          ).pipe(catchError(() => observableOf(null)));
        }),
        map(data => {
          this.isLoadingResults = false;
          this.isRateLimitReached = data === null;

          if (data === null) {
            return [];
          }
          this.resultsLength = data.length;
          return data;
        }),
      )
      .subscribe(data => {
        this.data = data;
        this.activeRow = this.data.find(x => x.id == 0);
      });
  }
  openDialog(): void {
    this.exampleDatabase = new EquipmentsHttpDatabase(this._httpClient);

    const dialogRef = this.dialog.open(EquipmentDialog, {
      data: {},
    });

    dialogRef.componentInstance._equipmentRepo = this.exampleDatabase;
    dialogRef.afterClosed().subscribe(result => {
      this.ngAfterViewInit();
    });
  }
}

export interface Equipment {
  id: number;
  name :string;
  sensors :SensorEntity[]
}

export class EquipmentsHttpDatabase {
  private authSecretKey = 'Bearer Token';
  private certKey = 'Cert'
  constructor(private _httpClient: HttpClient) {}
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
  getEquipmentsSensors(sort: string, order: SortDirection, page: number): Observable<Equipment[]> {
    const requestUrl = `${this.href}`;
    const headers = this.getHeaders();

    return this._httpClient.get<Equipment[]>(requestUrl, { headers });
  }
  getSensorsOfEquipment(id: number): Observable<Equipment> {
    const requestUrl = `${this.href}/${id}`;
    const headers = this.getHeaders();

    return this._httpClient.get<Equipment>(requestUrl,{ headers });
  }
  addOrEditSensor(Sensor: FormData): Observable<Equipment> {

    const recRef = 'https://vibroslleznov.zapto.org:4445/equipment';

    const requestUrl = `${recRef}`;
    const headers = this.getHeaders();

    return this._httpClient.post<Equipment>(requestUrl, Sensor,{ headers });
  }
}
