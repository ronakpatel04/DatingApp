import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent {

  baseUrl = "https://localhost:5001/api/";

  constructor(private http : HttpClient){}

  get404Error(){
    this.http.get(this.baseUrl+'buggy/not-found').subscribe({
      next:res=>console.log(res),
      error : error=>console.error(error)
      
    })
  }

    get400Error(){
      this.http.get(this.baseUrl+'buggy/bad-request').subscribe({
        next:res=>console.log(res),
        error : error=>console.error(error)
        
      })
    }
      get500Error(){
        this.http.get(this.baseUrl+'buggy/server-error').subscribe({
          next:res=>console.log(res),
          error : error=>console.error(error)
          
        })
      }
        get401Error(){
          this.http.get(this.baseUrl+'buggy/auth').subscribe({
            next:res=>console.log(res),
            error : error=>console.error(error)
            
          })
        } 
        get400ValidationError(){
          this.http.post(this.baseUrl+'account/register' ,{}).subscribe({
            next:res=>console.log(res),
            error : error=>console.error(error)
            
          })
        } 

   
  }


