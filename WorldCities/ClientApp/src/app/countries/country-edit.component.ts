import { Component, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { Country } from './../countries/Country';
import { CountryService } from './country.service';

import { BaseFormComponent } from '../base.form.component';

@Component({
  selector: 'app-country-edit',
  templateUrl: './country-edit.component.html',
  styleUrls: ['./country-edit.component.css']
})

export class CountryEditComponent extends BaseFormComponent
{
  title: string;
  form: FormGroup;
  country: Country;
  id?: number;

  // Activity Log (for debugging purposes)
  activityLog: string = '';

  constructor(private fb: FormBuilder,
            private activatedRoute: ActivatedRoute,
            private router: Router,
            private countryService: CountryService)
  {
    super();
    this.loadData();
  }

  ngOnInit()
  {
    this.form = this.fb.group({
      name: ['', Validators.required, this.isDupeField("name")],
      iso2: ['', [Validators.required, Validators.pattern('[a-zA-Z]{2}')], this.isDupeField("iso2")],
      iso3: ['', [Validators.required, Validators.pattern('[a-zA-z]{3}')], this.isDupeField("iso3")]
    });

    // react to form changes
    this.form.valueChanges
      .subscribe(val => {
        if (!this.form.dirty) {
          this.log("Form Model has been loaded.");
        }
        else {
          this.log("Form was updated by the user.");
        }
      });

    // react to changes in the form.name control
    this.form.get("name")!.valueChanges
      .subscribe(val => {
        if (!this.form.dirty) {
          this.log("Name has been loaded with initial values.");
        }
        else {
          this.log("Name was updated by the user.");
        }
      });

    this.loadData();
  }

  log(str: string) {
    this.activityLog += "["
      + new Date().toLocaleString()
      + "] " + str + "<br />";
  }

  loadData()
  {
    this.id = +this.activatedRoute.snapshot.paramMap.get('id');

    if (this.id)
    {
      //EDIT MODE
      this.countryService.get<Country>(this.id).subscribe(result =>
      {
        this.country = result;
        this.title = "Edit - " + this.country.name;
        this.form.patchValue(this.country);
      }, error => console.error(error));
    }
    else
    {
      //ADD NEW MODE
      this.title = "Create a new Country";
    }
  }

  onSubmit()
  {
    var country = (this.id) ? this.country : <Country>{};

    country.name = this.form.get("name").value;
    country.iso2 = this.form.get("iso2").value;
    country.iso3 = this.form.get("iso3").value;

    if (this.id)
    {
      //EDIT MODE

      this.countryService.put<Country>(country).subscribe(result =>
      {
        console.log("Country " + country.id + " has been updated.");
        this.router.navigate(['/countries']);
      }, error => console.log(error));
    }
    else
    {
      //ADD NEW MODE
      this.countryService.post<Country>(country).subscribe(result =>
      {
        console.log("Country " + result.id + " has been created.");
        this.router.navigate(['/countries']);
      }, error => console.log(error));
    }
  }

  isDupeField(fieldName: string): AsyncValidatorFn
  {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> =>
    {
      var countryId = (this.id) ? this.id.toString() : "0";

      return this.countryService.isDupeField(countryId, fieldName, control.value)
        .pipe(map(result =>
        {
          return (result ? { isDupeField: true } : null);
        }));
    }
  }
}
