# Modsen-Events
--------------------------------------------------
This is a complete solution of Events test task.
--------------------------------------------------

--------------------------------------------------
Deployment guide:

1. Make sure your device is connected to the Internet (docker-images taken from docker-hub)
2. (First time run) docker-compose build --parallel
3. docker-compose up -d 
The application has now been successfully deployed to a Docker container and is ready to process requests.



P.S 
There might be an issue if something has already occupied :443 port (proxy nginx is used as entry point to the app services) on Windows 10.
In my case Docker allowed nginx to start up on port 443 and requests couldn't reach it because they were sent to another app.
--------------------------------------------------



--------------------------------------------------
Valid request examples and server answers for each service are listed below


~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Auth service

Login:
![screenshot](Examples/Auth service/login.png)

Register:
![screenshot](Examples/Auth service/register.png)

Refresh:
![screenshot](Examples/Auth service/refresh.png)

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Data service

Create event:
![screenshot](Examples/Data service/create-event.png)

Update event:
![screenshot](Examples/Data service/update-event.png)

Get event by name:
![screenshot](Examples/Data service/get-event-by-name.png)

Get event by id:
![screenshot](Examples/Data service/get-event-by-id.png)

Get events (paged):
![screenshot](Examples/Data service/get-paged-events.png)

Get events (paged, by criteria):
![screenshot](Examples/Data service/get-event-by-criteria-1.png)
![screenshot](Examples/Data service/get-event-by-criteria-2.png)
![screenshot](Examples/Data service/get-event-by-criteria-3.png)

Save image:
![screenshot](Examples/Data service/save-event-image.png)

Get image: 
![screenshot](Examples/Data service/get-event-image.png)

Register participant:
![screenshot](Examples/Data service/register-participant.png)

Get participant by id:
![screenshot](Examples/Data service/get-event-participant-by-id.png)

Get event participants (paged):
![screenshot](Examples/Data service/get-paged-event-participants.png)

Delete event:
![screenshot](Examples/Data service/delete-event.png)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


--------------------------------------------------