import { Component } from '@angular/core';
import { ToastContainer } from './core/components/toast-container/toast-container';
import { Navbar } from './layout/components/navbar/navbar';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [ToastContainer, ToastContainer, Navbar, RouterOutlet],
})
export class App {}
