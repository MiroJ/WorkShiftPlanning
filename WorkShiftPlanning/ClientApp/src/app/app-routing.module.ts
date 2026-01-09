import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ScheduleInputComponent } from './components/schedule-input/schedule-input.component';
import { ScheduleResultsComponent } from './components/schedule-results/schedule-results.component';

const routes: Routes = [
  { path: '', component: ScheduleInputComponent },
  { path: 'results', component: ScheduleResultsComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
