# BlackPanther
This is a helper tool mostly used to compare server performance used for trading.Through this tool you can actually compare trader performance by comapring certain parameters of the trade like on an single instance a trade was pulled by two traders using single or different servers
#Programming Language used : C#,WPF
#WPF extensions : Mahapps
#Developed in : .Net4.5.2 and VS 2015
Requirement
HFT firms usually uses high end servers on which they run their trading software application used by the clients[Traders].On day to day basis the performance of the traders are monitored by the firm so they can
make changes to the server side /GUI side code to make the application more efficient and faster.
The main eyeball catcher here is how would you define performance of trade and on what parameters.HFT firms have their own ways to answer this question.In this article I am considering following scenarios to define the
traders performance.

1.On single server ON A GIVEN POINT OF TIME two traders runs a strategy on same symbol and expiry with only difference of bid and ask quote .
1.On single server ON A GIVEN POINT OF TIME two traders runs a strategy on same symbol and expiry with same bid and ask.
3.On different server ON A GIVEN POINT OF TIME two traders runs a strategy on same symbol and expiry with only difference of bid and ask quote .
4.On different server ON A GIVEN POINT OF TIME two traders runs a strategy on same symbol and expiry with same bid and ask.
5.When traders sits on a different application and at the same time using your application too.Then comparing the two applications performance.

When traders are compared on different servers then the servers too become the participants along with the traders.

This application compares traders/servers on above mention scenarios based on the unprocessed data that is fed to them.
At this moment application doesn't cater desire of comparing data in any format.Right now application is able to perform comparison on below file data format.

1.xml to xml comparison.
2.xml to database
3.database to database.
4.database to csv
