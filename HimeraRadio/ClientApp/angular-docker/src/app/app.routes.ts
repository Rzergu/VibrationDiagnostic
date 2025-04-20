import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EquipmentsComponent } from '../Components/Equipment/Equipment.component';
import { SensorAnaliticComponent } from '../app/sensor-analitic/sensor-analitic.component';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  {path: '', component:EquipmentsComponent, pathMatch:'full', canActivate: [AuthGuard]},
  {path: 'equipment', component:EquipmentsComponent, canActivate: [AuthGuard]},
  {path: 'sensorAnalitic', component:SensorAnaliticComponent},
  {path: '**', component:EquipmentsComponent, pathMatch:'full', canActivate: [AuthGuard]}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }