import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { UserIdleModule } from 'angular-user-idle';
//import { AngularFontAwesomeModule } from 'angular-font-awesome';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    //AngularFontAwesomeModule,
    UserIdleModule.forRoot({idle: 50, timeout: 50, ping:50})
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
