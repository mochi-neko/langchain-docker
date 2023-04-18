from fastapi import FastAPI
from langchain.llms import OpenAIChat
from langchain.memory import ConversationSummaryBufferMemory
from pydantic import BaseModel
from . import agent_with_google_search, conversational_agent, vector_store_agent

# Setup FastAPI
app = FastAPI()

# Setup langchain elements
llm = OpenAIChat()
memory = ConversationSummaryBufferMemory(llm=llm, memory_key="chat_history", return_messages=True)

# Root
@app.get("/")
def read_root():
    return {"Hello": "World"}

# Search agent
search_agent_executor = agent_with_google_search.setup_agent(llm=llm, memory=memory)

class SearchAgentRequest(BaseModel):
    content: str

@app.post("/agents/search")
async def search_agent(request: SearchAgentRequest):
    response = search_agent_executor.run(request.content)
    return {"response": response}

# Conversation agent
conversation_agent_executor = conversational_agent.setup_agent(llm=llm, memory=memory)

class ConversationAgentRequest(BaseModel):
    content: str

@app.post("/agents/conversation")
async def conversation_agent(request: ConversationAgentRequest):
    response = conversation_agent_executor.run(input=request.content, chat_history=memory.chat_memory)
    return {"response": response}

# Vector store agent
vector_store_agent_executor = vector_store_agent.setup_agent(llm=llm, memory=memory)

class VectorStoreAgentRequest(BaseModel):
    content: str

@app.post("/agents/vector-store")
async def vector_store_agent(request: VectorStoreAgentRequest):
    response = vector_store_agent_executor.run(request.content)
    return {"response": response}
