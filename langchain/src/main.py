from fastapi import FastAPI, Request
from pydantic import BaseModel
from . import agent_with_google_search

app = FastAPI()

class SearchAgentRequest(BaseModel):
    content: str

agent_executor = agent_with_google_search.setup_agent()

@app.get("/")
def read_root():
    return {"Hello": "World"}

@app.post("/agents/search")
async def search_agent(input_data: SearchAgentRequest):
    response = agent_executor.run(input_data.content)
    return {"response": response}
