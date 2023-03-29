import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../services/account.service';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  @ViewChild('loginForm') loginForm!: NgForm;
  logIn = false;
  name:string='';

  constructor(private route: Router, private accountService: AccountService,private toastr : ToastrService) {}

  ngOnInit(): void {
    this.getCurrentUser();
  }

  getCurrentUser() {
    this.accountService.currentUser$.subscribe({
      next: (user) => (this.logIn = !!user),
      error: (error) => console.log(error),
    });
  }
  login() {
    this.accountService.login(this.loginForm.value).subscribe({
      next: () => {
        this.name =  this.loginForm.value.username
        
        this.route.navigateByUrl('/members');
        this.logIn = true;
      },
      error: (error) => this.toastr.error(error.error),
    });
  }
  logout() {
    this.logIn = false;
    this.accountService.logout();
  }
}
