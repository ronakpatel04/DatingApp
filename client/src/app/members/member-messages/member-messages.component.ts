import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { Message } from 'src/app/models/message';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
})
export class MemberMessagesComponent implements OnInit {
    @Input() username ?: string;
   @Input() messages!:Message[]
  constructor(private messageService  :MessageService ){}
  
  ngOnInit(): void {
  }

  // loadMessages(){
  //   if(this.username){
  //    this.messageService.getMessageThread(this.username).subscribe(message =>{
  //       this.messages = message;
  //     })
  //   }
  }

