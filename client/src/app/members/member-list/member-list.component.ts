import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { Member } from 'src/app/models/member';
import { Pagination } from 'src/app/models/pagination';
import { User } from 'src/app/models/user';
import { UserParams } from 'src/app/models/userParams';
import { AccountService } from 'src/app/services/account.service';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  members: Member[] = [];
  pagination?: Pagination;
  userParams!: UserParams;
  user!: User;
  genderList = [{value:'male', display:'Males'}, {value:'female', display:'Females'}]
  constructor(
    private memberService: MembersService,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      if (user) {
        this.userParams = new UserParams(user);
        this.user = user;
      }
    });
  }
  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    if (!this.userParams) return;
    this.memberService.getMembers(this.userParams).subscribe((res) => {
      if (res.result && res.pagination) {
        this.members = res.result;
        this.pagination = res.pagination;
      }
    });
  }
  pageChange(event: any) {
    if (this.userParams && this.userParams.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.loadMembers();
    }
  }

  resetFilter(){
    if(this.user){
      this.userParams =  new UserParams(this.user);
      this.loadMembers();
    }
  }
}
