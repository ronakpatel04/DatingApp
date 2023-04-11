import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { User } from 'src/app/models/user';
import { AdminService } from 'src/app/services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css'],
})
export class UserManagementComponent implements OnInit {
  users: User[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> =  new BsModalRef<RolesModalComponent>();

  availableRoles = ['Admin', 'Moderator', 'Member'];
  constructor(
    private adminService: AdminService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUserWithRoles().subscribe((users) => {
      this.users = users;
    });
  }

  openRolesModal() {
    const initialState: ModalOptions = {
      initialState: {
        list: ['Do thing', 'Another thing', 'Something else'],
        title: 'Test Modal',
      },
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, initialState);
  }
}
