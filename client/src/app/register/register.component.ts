import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  @ViewChild('registerForm') registerForm!: NgForm;
  @Output() cancelRegister = new EventEmitter();

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}
  onRegister() {
    this.accountService.register(this.registerForm.value).subscribe({
      next: (res) => {
        console.log(res);
        this.onCancel();
      },
      error: (error) => { 
        if (error.error.errors.Username) {
          this.toastr.error(error.error.errors.Username);
        } else {
          this.toastr.error(error.error.errors.Password);
        }
      },
    });
  }

  onCancel() {
    this.cancelRegister.emit(false);
  }
}
