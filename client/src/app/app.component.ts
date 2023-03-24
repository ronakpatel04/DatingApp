import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  users:any;
  title = 'client';

  constructor(private http :  HttpClient){}

  ngOnInit(){
      this.http.get('https://localhost:7042/api/users').subscribe(res =>{
        this.users = res; 
      })
  }
}
