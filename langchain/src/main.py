from fastapi import FastAPI, Request
from pydantic import BaseModel
from langchain.llms import OpenAIChat
from langchain.memory import ConversationSummaryBufferMemory
from . import agent_with_google_search

app = FastAPI()

llm = OpenAIChat(modelName="gpt-3.5-turbo")
memory = ConversationSummaryBufferMemory(llm=llm, max_token_limit=300)
agent_executor = agent_with_google_search.setup_agent(llm=llm, memory=memory)

@app.get("/")
def read_root():
    return {"Hello": "World"}

class SearchAgentRequest(BaseModel):
    content: str

@app.post("/agents/search")
async def search_agent(request: SearchAgentRequest):
    response = agent_executor.run(request.content)
    return {"response": response}
