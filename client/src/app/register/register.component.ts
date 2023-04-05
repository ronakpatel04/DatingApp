import {
  Component,
  EventEmitter,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  NgForm,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm!: FormGroup;
  maxDate:Date =  new Date();   
  validationError!:string[]

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private route : Router
  ) {}
  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      knownAs: ['', Validators.required],

      gender: ['male'],

      dateOfBirth: ['', Validators.required],

      city: ['', Validators.required],
      country: ['', Validators.required],
      username: ['', Validators.required],
      password: [
        '',
        [Validators.required, Validators.minLength(4), Validators.maxLength(8)],
      ],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
    });
    this.registerForm.controls['password'].valueChanges.subscribe(() => {
      this.registerForm.controls['confirmPassword'].updateValueAndValidity;
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value == control.parent?.get(matchTo)?.value
        ? null
        : { notMatching: true };
    };
  }

  onRegister() {
      const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
      const values = {...this.registerForm.value, dateOfBirth: dob}

      

    this.accountService.register(values).subscribe({
      next: (res) => {
        this.route.navigateByUrl('/members')
       
      },
      error: (error) => {

    this.validationError = error;
    // <------  if (error.error.errors.Username) {
    //   this.toastr.error(error.error.errors.Username);
    // } else {
    //   this.toastr.error(error.error.errors.Password);
    // } ---->
    //   },
      }
    })
  }

  onCancel() {
    this.cancelRegister.emit(false);
  }


  private getDateOnly(dob:string | undefined){
    if(!dob) return;
    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset())).toISOString().slice(0,10);
  }
}
