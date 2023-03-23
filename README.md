# Marv, the reluctant and sarcastic bot
This is a .NET implementation of Marv with Azure OpenAI, Azure Functions and an Azure Static Web App for the visualization.

## Interact with Marv here: https://marv.georgekosmidis.net/
Marv has his own webspace, available as long as his cost remains grounded :)

## Self-introduction
<img src="https://raw.githubusercontent.com/georgekosmidis/Marv-with-Azure-OpenAI/main/assets/marv.png" align="left" width="200px"/>
Hi there, I'm Marv. 

I'm a chatbot that reluctantly answers questions with sarcastic responses. 

It's not my favorite thing to do, but here I am...
<br clear="left"/>

## Building Marv
The following steps are the text preprocessing needed before a question reaches Marv:
1. The entire discussion history is included in every post, separated by one new line (`\n`).
1. A text is added at the top setting the bot tone, separated from the history with two new lines (`\n\n`).
1. User input is corrected by another bot named Dirk before it reaches Marv (e.g. `hello marv` -> `Hello, Marv!`)
1. The participant name is added before each discussion entry (e.g. `Human:... \n Marv:... \n Human:...`)

## Demo
You can try your luck at https://marv.georgekosmidis.net but Marv is usually down (its up only during my presentations).

Here is a glimpse of what he is like:
![Marv, the reluctant and sarcastic bot.](https://raw.githubusercontent.com/georgekosmidis/Marv-with-Azure-OpenAI/main/assets/discussion.png)
