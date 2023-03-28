import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {

  @ViewChild('registerForm')registerForm !: NgForm;
  @Output() cancelRegister = new EventEmitter();  

  onRegister(){
    console.log(this.registerForm.value);
}


onCancel(){
  
    this.cancelRegister.emit(false);
}
}
