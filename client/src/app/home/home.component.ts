import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  registerMode = false;

  registerToggle(){
    this.registerMode =  !this.registerMode
  }

  onCancelEvent(data:boolean){
      this.registerMode = data;
  }

 
}
