import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { BaseFormComponent } from './base.form.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CityService } from './cities/city.service';
import { CitiesComponent } from './cities/cities.component';
import { CityEditComponent } from './cities/city-edit.component';
import { CountryService } from './countries/country.service';
import { CountriesComponent } from './countries/countries.component';
import { CountryEditComponent } from './countries/country-edit.component';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialModule } from './angular-material.module';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

import { ApiAuthorizationModule } from './api-authorization/api-authorization.module';
import { AuthorizeGuard } from './api-authorization/authorize.guard';
import { AuthorizeInterceptor } from './api-authorization/authorize.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    BaseFormComponent,
    NavMenuComponent,
    HomeComponent,
    CitiesComponent,
    CityEditComponent,
    CountriesComponent,
    CountryEditComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ApiAuthorizationModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'countries', component: CountriesComponent },
      { path: 'country/:id', component: CountryEditComponent, canActivate: [AuthorizeGuard] },
      { path: 'country', component: CountryEditComponent, canActivate: [AuthorizeGuard] },
      { path: 'cities', component: CitiesComponent },
      { path: 'city/:id', component: CityEditComponent, canActivate: [AuthorizeGuard] },
      { path: 'city', component: CityEditComponent, canActivate: [AuthorizeGuard] }
    ]),
    BrowserAnimationsModule,
    AngularMaterialModule,
    ReactiveFormsModule
  ],
  providers: [
    CityService,
    CountryService,
    {
      provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
