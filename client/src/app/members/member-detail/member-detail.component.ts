import {
  AfterViewInit,
  Component,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryOptions,
} from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs';
import { Member } from 'src/app/models/member';
import { Message } from 'src/app/models/message';
import { User } from 'src/app/models/user';
import { AccountService } from 'src/app/services/account.service';
import { MembersService } from 'src/app/services/members.service';
import { MessageService } from 'src/app/services/message.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  member!: Member;
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?: User;

  constructor(
    private accountService: AccountService,
    private route: ActivatedRoute,
    private messageService: MessageService,
    public presenceService: PresenceService,
    private memberService: MembersService,
    private router: Router
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      if (user) this.user = user;
    });
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
  ngOnInit(): void {
    this.loadMember();
    this.route.data.subscribe((data) => {
      this.member = data['member'];
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imagePercent: 100,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false,
      },
    ];

    this.route.queryParams.subscribe((params) => {
      params['tab'] && this.selectTab(params['tab']);
    });
  }
  getImages() {
    if (!this.member) return [];
    const imageUrls = [];
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
      });
    }
    return imageUrls;
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs!.tabs.find((x) => x.heading == heading)!.active = true;
    }
  }

  loadMember() {
    var username = this.route.snapshot.paramMap.get('username');
    if (!username) return;
    this.memberService.getMember(username).subscribe((member) => {
      this.galleryImages = this.getImages();
      // console.log(this.galleryImages);
    });
  }

  loadMessages() {
    if (this.member) {
      this.messageService
        .getMessageThread(this.member.userName)
        .subscribe((message) => {
          this.messages = message;
        });
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Message' && this.user) {
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }
}
