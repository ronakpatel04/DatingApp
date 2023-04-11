import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) {}

  getUserWithRoles()
  {
    return this.http.get<User[]>(this.baseUrl+'admin/users-with-roles');
  }
}
