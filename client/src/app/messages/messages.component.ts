import { Component, OnInit } from '@angular/core';
import { Message } from '../models/message';
import { Pagination } from '../models/pagination';
import { MessageService } from '../services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css'],
})
export class MessagesComponent implements OnInit {
  messages?: Message[];
  pagination?:Pagination;
  container ='Outbox';
  pageNumber =1;
  pageSize =5;
  constructor(private messageService :MessageService) {}

  ngOnInit(): void {
    this.loadMessage(); 
  }


  loadMessage(){
    this.messageService.getMessages(this.pageNumber,this.pageSize,this.container).subscribe(res=>
      {
        this.messages =res.result;
        this.pagination = res.pagination;
      })
  }

  pageChanged(event:any){
    if(this.pageNumber !== event.page)
    {
      this.pageNumber = event.page;
    }
  }


}
