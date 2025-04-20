import {ChangeDetectionStrategy, Component, inject, model,Input, signal} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatCheckboxModule} from '@angular/material/checkbox';
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
import {NgxMaterialTimepickerModule} from 'ngx-material-timepicker';
import {MatInputModule} from '@angular/material/input';
import { SensorEntity, SensorsHttpDatabase } from '../Sensors.component';
import { MaterialFileInputModule } from 'ngx-material-file-input';

/**
 * @title Dialog Overview
 */
@Component({
  selector: 'SensorDialog',
  templateUrl: 'SensorDialog.component.html',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    MaterialFileInputModule,
    ReactiveFormsModule,
    MatCheckboxModule,
    MatDialogTitle,
    MatDialogContent,
    NgxMaterialTimepickerModule,
    MatDialogActions,
    MatDialogClose
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})

export class SensorDialog {
  readonly dialogRef = inject(MatDialogRef<SensorDialog>);
  readonly data = inject<SensorEntity>(MAT_DIALOG_DATA);
  public _SensorsRepo: SensorsHttpDatabase | null;
  public equipmentId : number | null;

  fileForm : FormGroup;
  constructor(private fb: FormBuilder) {
    this.fileForm = this.fb.group({
        "title": ['', Validators.required]
    });
  }
  onNoClick(): void {
    this.dialogRef.close();
  }
  submit(){
    console.log('The dialog was closed');
    const formData = new FormData();

    formData.append('date', new Date().toDateString());
    formData.append('title', this.fileForm.value.title);
    formData.append('equipmentId', this.equipmentId != null ? this.equipmentId.toString() : "0");

    this._SensorsRepo!.addOrEditSensor(
      formData
    ).subscribe(res => {
      this.dialogRef.close();
    });
    //this.dialogRef.close();
  }
}


/**  Copyright 2024 Google LLC. All Rights Reserved.
    Use of this source code is governed by an MIT-style license that
    can be found in the LICENSE file at https://angular.io/license */