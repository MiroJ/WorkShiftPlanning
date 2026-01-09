import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ScheduleInputComponent } from './components/schedule-input/schedule-input.component';
import { ScheduleResultsComponent } from './components/schedule-results/schedule-results.component';

@NgModule({
  declarations: [
    AppComponent,
    ScheduleInputComponent,
    ScheduleResultsComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
