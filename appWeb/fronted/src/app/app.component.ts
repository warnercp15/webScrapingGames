import { Component } from '@angular/core';
import { HttpErrorResponse, HttpClient } from '@angular/common/http';
import { UserIdleService } from 'angular-user-idle'; 
const Swal = require('sweetalert2')

// CommonJS
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  	title = 'viewGames';

	listaJuegos:any=[];

	Toast = Swal.mixin({
		toast: true,
		position: 'top-end',
		showConfirmButton: false,
		timer: 3000,
		timerProgressBar: true,
		didOpen: (toast) => {
			toast.addEventListener('mouseenter', Swal.stopTimer)
			toast.addEventListener('mouseleave', Swal.resumeTimer)
		}
	});

	constructor(
		private _http: HttpClient,
		public userIdle: UserIdleService
	) {}

	ngOnInit() {
		this.getData();
		this.userIdle.startWatching();
		this.userIdle.ping$.subscribe(() => {
			this.getData();
			this.Toast.fire({
				icon: 'success',
				title: "Datos actualizados"
			});
		});
	}

   	getData() {
		this._http.get("tuIpv4/getData").subscribe(
		data => (
			this.listaJuegos=data),
		(err: HttpErrorResponse) => {
			console.log(err)
		})
	}
}
