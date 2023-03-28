import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  @ViewChild('loginForm') loginForm!: NgForm;
  logIn = false;

  constructor(private route: Router, private accountService: AccountService) {}

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
      next: (response) => {
        console.log(response);
        this.logIn = true;
      },
      error: (error) => console.error(error),
    });
    this.loginForm.reset();
  }
  logout() {
    this.logIn = false;
    this.accountService.logout();
  }
}
