import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from '../models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);  
  onlineUser$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService, private route : Router) {}

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));

    this.hubConnection.on('UserIsOnline', (username) => {
      this.toastr.info(username + ' has connected');
    });

    this.hubConnection.on('UserIsOffline', (username) => {
      this.toastr.warning(username + ' has disconnected');
    });


      this.hubConnection.on('GetOnlineUsers', usernames=>{
        this.onlineUsersSource.next(usernames);
      })


        this.hubConnection.on('NewMessageReceived',({username, knownAs})=>{
          this.toastr.info(knownAs+' has sent you a new message! Click me to see it ').onTap.pipe(take(1)).subscribe(()=>{
                this.route.navigateByUrl('/members/'+username+'?tab=Message')
          })
        })

  }

    stopHubConnection()
    {
      this.hubConnection?.stop().catch(error => console.log(error))
    }


}