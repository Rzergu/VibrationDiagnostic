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
import { Equipment, EquipmentsHttpDatabase } from '../Equipment.component';
import { MaterialFileInputModule } from 'ngx-material-file-input';

/**
 * @title Dialog Overview
 */
@Component({
  selector: 'equipmentDialog',
  templateUrl: 'EquipmentDialog.component.html',
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

export class EquipmentDialog {
  readonly dialogRef = inject(MatDialogRef<EquipmentDialog>);
  readonly data = inject<Equipment>(MAT_DIALOG_DATA);
  public _equipmentRepo: EquipmentsHttpDatabase | null;

  fileForm : FormGroup;
  constructor(private fb: FormBuilder) {
    this.fileForm = this.fb.group({
        "name": ['', Validators.required]
    });
  }
  onNoClick(): void {
    this.dialogRef.close();
  }
  submit(){
    console.log('The dialog was closed');
    const formData = new FormData();
    formData.append('name', this.fileForm.value.name);

    this._equipmentRepo!.addOrEditSensor(
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