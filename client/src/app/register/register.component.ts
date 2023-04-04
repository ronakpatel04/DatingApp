import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgForm, ValidatorFn, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit{
  @Output() cancelRegister = new EventEmitter();
  registerForm!: FormGroup;



  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}
  ngOnInit(): void {
        this.initializeForm();
  }
  
  initializeForm(){
    this.registerForm = new FormGroup({
        username : new FormControl('', Validators.required),
        password:new FormControl('', [Validators.required ,Validators.minLength(4), Validators.maxLength(8)]),
        confirmPassword: new FormControl('',[Validators.required,this.matchValues('password')])

    })
    this.registerForm.controls['password'].valueChanges.subscribe(()=>{
      this.registerForm.controls['confirmPassword'].updateValueAndValidity
    })
  }

  matchValues(matchTo:string):ValidatorFn
  {
    return  (control:AbstractControl) =>{
      return control.value == control.parent?.get(matchTo)?.value ?null: {notMatching:true}
    }
  }
  
  
  
  onRegister() {



    console.log(this.registerForm.value)

    // this.accountService.register(this.registerForm.value).subscribe({
    //   next: (res) => {
    //     console.log(res);
    //     this.onCancel();
    //   },
    //   error: (error) => { 
      
    //     console.log(error);
    //     this.toastr.error(error);
        //<------  if (error.error.errors.Username) {
        //   this.toastr.error(error.error.errors.Username);
        // } else {
        //   this.toastr.error(error.error.errors.Password);
        // } ---->
    //   },
    // });
  }

  onCancel() {
    this.cancelRegister.emit(false);
  }
}
