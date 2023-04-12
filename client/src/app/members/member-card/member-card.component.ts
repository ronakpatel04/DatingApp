import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/models/member';
import { MembersService } from 'src/app/services/members.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent implements OnInit {
  @Input() member!: Member;
  ngOnInit(): void {}

  constructor(
    private memberService: MembersService,
    private toastr: ToastrService,
    public presenceService : PresenceService
  ) {}

  addLike(member: Member) {
    this.memberService.addLike(member.userName).subscribe((res) => {
      this.toastr.success('You have liked ' + member.knownAs);
    });
  }
}
