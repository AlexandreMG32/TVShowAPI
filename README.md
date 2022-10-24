# TVShowAPI

## An API to search for your favorite TV shows, actors and episodes

### API made using .NET, Entity Framework and powered by Swagger UI, for better user experience

![API get example](https://user-images.githubusercontent.com/33472945/197348946-843033fb-12fa-4f61-9c63-c84e0933791a.png)

> #### How to install and run the API
> First, clone this project, using git.
> Second, make sure you have .NET SDK and SqlServer installed
> Third, open a bash/cmd on the folder CodeChallenge, of the project
> Update the database with the migration given
> ```
> dotnet ef database update
> ```
> Next, run the project
> ```
> dotnet run
> ```
> After this, open your browser at https://localhost:7140/swagger/index.html
> The API is pretty self explanatory using the Swagger GUI, except for one part that I will now explain

> #### Register, Login and Favorites
> The registration and login is self explanatory, enter a username and password for registration, if the username
> isnt unique you will get an error.
> After a successfull registration, login with the same credentials, and you will get as a response body, the token you need
> to authenticate
> ![image](https://user-images.githubusercontent.com/33472945/197622640-0de628ca-68e1-4b13-ad12-487137b8fc15.png)
> With this token, go to the top of the page and click on authenticate
> In this field, type "bearer" plus your copied token
> ![image](https://user-images.githubusercontent.com/33472945/197623131-1f360bed-c6ef-4b0f-b35a-8aca1067c83f.png)
> You are now authenticated and can add, remove and see your favorite shows.
> Doing these is self-explanatory too, just use the id of the show you want to add or remove

