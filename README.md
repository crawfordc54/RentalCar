Purpose
The purpose of this assignment is to integrate various concepts into a real-world style scenario for production application maintenance.

Objective
Dave's Rental Car Emporium is a small rental car agency just outside of Columbus, Ohio, that was founded in late 2017. It specialises in medium-to-long term rentals (greater than five days at a time). The typical customer of Dave's rents a car for travel while in town for a conference, residency at one of the area educational institutions. In addition, Dave's has a working relationship with the State of Ohio to provide rental cars for people whose vehicles have been temporarily impounded due to it being involved in a legal situation. Dave's also has a working relationship with several exotic- or rare-car dealerships in the greater Columbus area to provide loaner vehicles in the event a rare vehicle rqeuires a long-term repair. The majority of Dave's customers have the bills paid by a third party, such as a university, government, or insurance company.

Shortly before Dave's opened, a contractor quickly put together a .NET console application to manage the basic rental functions. For awhile, the software served the needs of the business. However, over time, there has been a need to make some bug fixes and enhancements. However, the original developer is nowhere to be found. You have been hired to help resolve some of the backlog of items for the rental car system. The list, provided by Dave, and maintained in Excel, includes the following items he'd like you to work on first:

#	Type	      Priority	Description

3	Enhancement	High	I was going through some old records and I saw that a reservation that was self-pay for one of our customers, Wendy Copeland, but Wendy was under 25 at the time. Our insurance company won't allow us to do self-pay for people under 25, because of liability. If someone else is paying, like insurance or the school, it's fine, but we can't have self-pay for customers under 25. Can you add some sort of check to make sure we can't reserve for someone under 25?

4	Bug	        High	One of our promotional flyer sent to customers was sent back as undeliverable by the post office. It says that the ZIP code is bad, but I don't understand why. I put it on the envelope exactly as the screen said? The customer's name is Glenn Pogue.

5	Enhancement	High	We give our self-pay customers a 15% discount, because we don't have to deal with insurance and all of that. The agents are trained to do the math and put it in the trip's rate, but they forgot recently with one of our regular customers, Bill Greene. I called him and said I was sorry and credited his card, but I'd like to make sure this doesn't happen again. Can you make it where if it's a self-pay, it automatically gives the customer a 15% discount?

6	Enhancement	Medium	Right now, agents need to see me to reset their system password, which our credit card people say they need to do every month. Can you add a menu item to allow them to do it themselves?

7	Enhancement	Medium	The vehicle screen shows all of the vehicles, but it doesn't tell me which ones have been disposed of. Can you add a column to show the disposal date?

8	Enhancement	Medium	The commission report is year-to-date, which is OK, except if I need to do some checking for tax reports. Can you make it work where I can type in the dates that the report will run?

9	Bug	        Medium	The reservation entry screen makes you key in the customer number when creating a reservation. You have to go out to the customer search, find the customer ID or add them, write down the number, and then go to the reservation and key in that number. If you don't put the number in correctly, it'll crash the system. Ideally, it would be great if you could have it do the search when you're creating the reservation, but can you at least stop it from crashing if you don't type in the right number?
