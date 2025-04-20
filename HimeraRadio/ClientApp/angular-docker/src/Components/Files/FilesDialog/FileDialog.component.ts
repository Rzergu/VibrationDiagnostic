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
import {MatInputModule} from '@angular/material/input';
import { FileEntity, FilesHttpDatabase } from '../Files.component';
import { MaterialFileInputModule } from 'ngx-material-file-input';

export interface DialogData {
  file: FileEntity;
  isEdit: Boolean;
}
/**
 * @title Dialog Overview
 */
@Component({
  selector: 'filesDialog',
  templateUrl: 'FileDialog.component.html',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    MaterialFileInputModule,
    ReactiveFormsModule,
    MatDialogTitle,
    MatDialogContent,
    MatCheckboxModule,
    MatDialogActions,
    MatDialogClose,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})


export class FileDialog {
  readonly dialogRef = inject(MatDialogRef<FileDialog>);
  readonly data = inject<DialogData>(MAT_DIALOG_DATA);
  public _filesRepo: FilesHttpDatabase | null;
  public SensorId : number | null;

  fileForm : FormGroup;

  constructor(private fb: FormBuilder) {
    if(this.data.isEdit)
    {
      this.fileForm = this.fb.group({
        "name": [this.data.file.name, Validators.required],
        "file": [''],
        "isLatest": [this.data.file.isLatest],
      });
    }
    else {
      this.fileForm = this.fb.group({
        "name": ['', Validators.required],
        "file": ['', Validators.required],
        "isLatest": [false],
      });
    }
  }
  onNoClick(): void {
    this.dialogRef.close();
  }
  submitSave(){
    console.log('The dialog was closed');
    const formData = new FormData();

    formData.append('file', this.fileForm.value.file.files[0], this.fileForm.value.file.files[0].name,);
    formData.append('name', this.fileForm.value.name.toString());
    formData.append('isLatest', this.fileForm.value.isLatest);
    formData.append('SensorId', this.SensorId != null ? this.SensorId.toString() : "0");
    formData.append('fileName', this.fileForm.value.file.files[0].name);

    this._filesRepo!.addFile(
      formData
    ).subscribe(res => {
      this.dialogRef.close();
    });
  }
  submitEdit(){
    console.log('The dialog was closed');
    const formData = new FormData();

    formData.append('name', this.fileForm.value.name.toString());
    formData.append('isLatest', this.fileForm.value.isLatest);
    formData.append('SensorId', this.SensorId != null ? this.SensorId.toString() : "0");

    this._filesRepo!.editFile(
      formData, this.data.file.id
    ).subscribe(res => {
      this.dialogRef.close();
    });
  }
}