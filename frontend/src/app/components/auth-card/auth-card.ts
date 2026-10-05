import { Component, input } from '@angular/core';

@Component({
  selector: 'app-auth-card',
  templateUrl: './auth-card.html',
  styleUrl: './auth-card.css',
})
export class AuthCard {
  title = input.required<string>();
  subtitle = input('');
}
