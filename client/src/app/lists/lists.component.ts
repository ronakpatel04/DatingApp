import { Component, OnInit } from '@angular/core';
import { Member } from '../models/member';
import { MembersService } from '../services/members.service';
import { Pagination } from '../models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'],
})
export class ListsComponent implements OnInit {
  members!: Member[];
  predicate = 'liked';
  pageNumber =1;
  pageSize=6;
  pagination!:Pagination
  constructor(private memberService: MembersService) {}
  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    this.memberService.getLikes(this.predicate,this.pageNumber,this.pageSize).subscribe((res) => {
             this.members = res.result!;
             this.pagination = res.pagination!;
    });
  }

  pageChange(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadLikes();
    }
  }



}
