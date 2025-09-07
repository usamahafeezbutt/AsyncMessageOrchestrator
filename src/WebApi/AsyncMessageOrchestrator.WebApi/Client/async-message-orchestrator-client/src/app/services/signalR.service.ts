import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { Router } from '@angular/router';
import { MessageDto } from '../dtos/MessageDto';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class SignalRService {
    private hubConnection: HubConnection;
    private notificationsSubject = new BehaviorSubject<MessageDto>(null);
    private resetNotificationCounterSubject = new BehaviorSubject<boolean>(false);
    public notifications$ = this.notificationsSubject.asObservable();
    public notificationsCounter$ = this.resetNotificationCounterSubject.asObservable();

    constructor(
        private router: Router) { }

    public startConnection() {

        this.hubConnection = new HubConnectionBuilder()
            .withUrl(
                environment.API_BASE_URL + `/notificationHub`, {
                transport: signalR.HttpTransportType.WebSockets,
                withCredentials: true
            })
            .withAutomaticReconnect()
            .build();

        // Set the server timeout (default is 30,000 milliseconds)
        this.hubConnection.serverTimeoutInMilliseconds = 10000000000000; 

        // Set the keep alive interval (default is 15,000 milliseconds)
        this.hubConnection.keepAliveIntervalInMilliseconds = 10000000000000; 

        this.hubConnection
            .start()
            .then(() => {})
            .catch(err => {});

        this.hubConnection.on('ReceiveNotification', (message) => {
           alert(message.message);
        });
    }

    public stopConnection() {
        if (this.hubConnection) {
            this.hubConnection.stop()
                .then(() => console.log('Connection stopped'))
                .catch(err => console.log('Error while stopping connection: ' + err));
        }
    }
}
