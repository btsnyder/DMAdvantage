# DMAdvantage
While running my D&D campaign I thought that it would be fun to use some of my programming knowledge to build a tool for me to use. The backend is an ASP.NET project with a fairly straightforward REST API. The frontend is built in Blazor and relying on Mudblazor (a component library) for some of the basic UI. The data is being stored in a SQL database. While I did get this setup to run in Azure, due to costs and this just being a personal project, I decided to move things back to being locally hosted.
## Player vs DM
The user for the application is inteded to be the DM (Dungeon Master). They are able to enter in all the data that they need to run a successful campaign. The main draw of this is to track and manage encounters. Before I was using multiple different apps to manage my data and also had a lot of custom elements, because it was Star Wars themed. The app will allow for entry of multiple different types of data to run the encounter (characters, enemies, weapons, etc.). For the players, there are view pages that do not require authentication and will not change the data. This allows players to track along with the encounter.
## Testing
As a part of any project, I found it important to test my code. While a strench goal for me is to eventually build out UI tests, for now I focused on testing the API.
## New Technologies
This project allowed me to learn and implement technologies that I was working with to deepen my knowledge
### Entity Framework
All my data was managed by EF. While I began using a Repository pattern, I instead used EF directly so that I could learn exactly how EF managed it.
### Kafka
To live update my view whenever the encounter is altered, I implemented a fairly simple Kafka pattern to send an update to the page to relead the data.
### Azure
For a breif time (as long as I had a free trial) I hosted this site on Azure. This gave me practice with the deployment process, as well as the key manager since I was making it publically available.
