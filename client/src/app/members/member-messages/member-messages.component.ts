import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/models/message';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm!: NgForm;
  @Input() username?: string;
  messageContent = '';
  constructor(public messageService: MessageService) {}

  ngOnInit(): void {}

  sendMessage() {
    console.log(this.username)
    if (!this.username) return;

    this.messageService
      .sendMessage(this.username, this.messageContent).then(()=>{
        this.messageForm.reset();
      })
      // .subscribe((message) => {
      //   this.messages.push(message);
      //   this.messageForm.reset();
      // });
  }
}
