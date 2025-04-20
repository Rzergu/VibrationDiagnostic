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
import { SensorEntity } from '../Sensors/Sensors.component';
import {MatIconModule} from '@angular/material/icon';
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
import { FileDialog } from './FilesDialog/FileDialog.component';
import { SensorsComponent } from '../Sensors/Sensors.component';
import { saveAs } from "file-saver";
import * as CryptoJS from "crypto-js";
import { Router } from '@angular/router';


@Component({
  selector: 'files',
  styleUrl: 'Files.component.scss',
  templateUrl: 'Files.component.html',
  standalone: true,
  imports: [MatProgressSpinnerModule, MatIconModule, MatTableModule, MatSortModule, MatPaginatorModule, DatePipe,
            MatFormFieldModule, MatInputModule, FormsModule, MatButtonModule],
})
export class FilesComponent implements AfterViewInit {
  @Input() SensorId:number|null
  displayedColumns: string[] = ['name', 'isLatest', 'download', 'edit', 'gotoAnalitic', 'delete'];
  filesDatabase: FilesHttpDatabase | null;
  data: FileEntity[] = [];
  readonly dialog = inject(MatDialog);
  resultsLength = 0;
  isLoadingResults = true;
  initialized = false;
  isRateLimitReached = false;
  activeRow:FileEntity | null | undefined = null;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private _httpClient: HttpClient, private router: Router) {}

  ngOnChanges()
  {
    if(this.initialized)
      {
        this.isLoadingResults = true;
        return this.filesDatabase!.getRepoFilesBySensor(
          this.SensorId
        ).pipe(catchError(() => observableOf(null))).subscribe(data => {
          if(data !== null)
          {
            this.data = data;
            this.isLoadingResults = false;
            this.activeRow = data[0];
          }
          return;
        });
      }
      return;
  }
  ngAfterViewInit() {
    this.filesDatabase = new FilesHttpDatabase(this._httpClient);
    this.initialized = true;
    this.SensorId = 1;
  }
  openDialog(): void {
    const dialogRef = this.dialog.open(FileDialog, {
      data: {},
    });
    dialogRef.componentInstance._filesRepo = this.filesDatabase;
    dialogRef.componentInstance.SensorId = this.SensorId;
    dialogRef.afterClosed().subscribe(result => {
      this.ngOnChanges();
    });
  }
  gotoAnalitic(file:any) : void {
    this.filesDatabase!.downloadFile(file.id)
      .subscribe(async (fileData:any) =>
        {
            const arrayBuffer = await fileData.arrayBuffer();  // Convert Blob to ArrayBuffer
            const encryptedBytes = new Uint8Array(arrayBuffer);
            const encryptedWordArray = this.byteArrayToWordArray(encryptedBytes);
            const cipherParams = CryptoJS.lib.CipherParams.create({
              ciphertext: encryptedWordArray
            });

            const key = CryptoJS.enc.Utf8.parse("119477C6704319884D91AFF61C592E16");
            const iv = CryptoJS.enc.Utf8.parse("");

            const decryptedWordArray = CryptoJS.AES.decrypt(
              { ciphertext: encryptedWordArray } as any,
              key,
              { iv: iv }
            );
            var data = this.wordArrayToByteArray(decryptedWordArray);
            var string = new TextDecoder().decode(data);

            this.router.navigate(['/sensorAnalitic'],{ state: { sensorState: string } });
        });
  }
  download(file: any) : void {
    this.filesDatabase!.downloadFile(file.id)
      .subscribe(async (fileData:any) =>
        {
            const arrayBuffer = await fileData.arrayBuffer();  // Convert Blob to ArrayBuffer
            const encryptedBytes = new Uint8Array(arrayBuffer);
            const encryptedWordArray = this.byteArrayToWordArray(encryptedBytes);
            const cipherParams = CryptoJS.lib.CipherParams.create({
              ciphertext: encryptedWordArray
            });

            const key = CryptoJS.enc.Utf8.parse("119477C6704319884D91AFF61C592E16");
            const iv = CryptoJS.enc.Utf8.parse("");

            const decryptedWordArray = CryptoJS.AES.decrypt(
              { ciphertext: encryptedWordArray } as any,
              key,
              { iv: iv }
            );
            var data = this.wordArrayToByteArray(decryptedWordArray);
            saveAs( new Blob([data], { type: 'text/plain' }), file.fileName)
        });
  }

  edit(file: any) : void {
    const dialogRef = this.dialog.open(FileDialog, {
      data: {file: file, isEdit: true}
    });
    dialogRef.componentInstance._filesRepo = this.filesDatabase;
    dialogRef.componentInstance.SensorId = this.SensorId;
    dialogRef.afterClosed().subscribe(result => {
      this.ngOnChanges();
    });
  }
  byteArrayToWordArray(u8Array: Uint8Array): CryptoJS.lib.WordArray {
    const words = [];
    for (let i = 0; i < u8Array.length; i += 4) {
      // Combine 4 bytes into one 32-bit word.
      words.push(
        (u8Array[i] << 24) |
        ((u8Array[i + 1] || 0) << 16) |
        ((u8Array[i + 2] || 0) << 8) |
        ((u8Array[i + 3] || 0))
      );
    }
    return CryptoJS.lib.WordArray.create(words, u8Array.length);
  }
  wordArrayToByteArray(wordArray: CryptoJS.lib.WordArray): Uint8Array {
    const { words, sigBytes } = wordArray;
    const u8Array = new Uint8Array(sigBytes);
    let index = 0;
    for (let i = 0; i < sigBytes; i++) {
      // Determine which word and byte in that word we are on.
      const word = words[Math.floor(i / 4)];
      u8Array[i] = (word >> (24 - (i % 4) * 8)) & 0xff;
    }
    return u8Array;
  }
  deleteRow(file: any) : void {
    this.filesDatabase!.deleteFile(file.id)
      .subscribe((res:any) =>
        {
          this.ngOnChanges();
        });
  }
}

export interface FileEntity {
  id: number;
  name: string;
  file :File;
  fileName: string;
  isLatest: number;
}


export class FilesHttpDatabase {
  private authSecretKey = 'Bearer Token';
  private certKey = 'Cert';
  constructor(private _httpClient: HttpClient) {}
  href = 'https://vibroslleznov.zapto.org:4445/files';
  private getHeaders(): HttpHeaders {
    const authToken = localStorage.getItem(this.authSecretKey);
    const certToken = localStorage.getItem(this.certKey);
    var httpHeaders = new HttpHeaders({
      Authorization: `Bearer ${authToken}`
    });
    httpHeaders = httpHeaders.set('CertificateKey',  `${certToken}`);
    return httpHeaders;
  }
  getRepoFiles(sort: string, order: SortDirection, page: number): Observable<FileEntity[]> {
    const requestUrl = `${this.href}`;

    const headers = this.getHeaders();
    return this._httpClient.get<FileEntity[]>(requestUrl);
  }
  getRepoFilesBySensor(SensorId: number|null): Observable<FileEntity[]> {
    const href = 'https://vibroslleznov.zapto.org:4445/Sensors';
    var requestUrl = `${href}`;
    if(SensorId != null)
    {
        requestUrl =  `${href}/${SensorId}`;
    }

    const headers = this.getHeaders();
    var Sensor = this._httpClient.get<SensorEntity>(requestUrl, { headers });
    return Sensor.pipe(map(x=> x.files.map(e => {
      return e;
    })))
  }
  addFile(filter: FormData): Observable<FileEntity> {

    const requestUrl = `${this.href}`;

    const headers = this.getHeaders();
    return this._httpClient.post<FileEntity>(requestUrl, filter, { headers });
  }
  editFile(filter: FormData, fileId: number): Observable<FileEntity> {

    const requestUrl = `${this.href}`;

    const headers = this.getHeaders();
    return this._httpClient.post<FileEntity>(requestUrl+'/edit/'+fileId, filter, { headers });
  }
  downloadFile(fileId: number) {

    const headers = this.getHeaders();
    return this._httpClient.get('https://vibroslleznov.zapto.org:4445/files/'+fileId, { responseType: 'blob', headers });
  }
  deleteFile(fileId: number) {

    const headers = this.getHeaders();
    return this._httpClient.post('https://vibroslleznov.zapto.org:4445/files/delete/'+fileId, {headers});
  }
}

