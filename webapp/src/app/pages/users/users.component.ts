import { Component, OnInit } from '@angular/core'
import { User } from 'src/app/models'
import { UserService } from 'src/app/services'

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {

  constructor(private usersService: UserService) { }

  public users: Array<User> = [];

  ngOnInit(): void {
    this.usersService.getAll().subscribe(users => {
      this.users = users;
    });
  }

}
