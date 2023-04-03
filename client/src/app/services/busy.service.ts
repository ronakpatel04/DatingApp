import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {

  constructor(private spinnerService:NgxSpinnerService) { }

  busyRequestcount = 0;

  busy(){
    this.busyRequestcount++;
    this.spinnerService.show(undefined,{
      type:'line-scale-party',
      bdColor:'rgba(255,255,255,0.05)',
      color:'#fff'

    })
  }


  idle(){
    this.busyRequestcount--;
    if(this.busyRequestcount<=0){
      this.busyRequestcount =0 ;
      this.spinnerService.hide();
    }
  }
}
