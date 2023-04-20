from fastapi import FastAPI
from langchain.llms import OpenAIChat
from langchain.memory import ConversationSummaryMemory
from pydantic import BaseModel

from .agents import conversational_agent

# Setup FastAPI
app = FastAPI()

# Setup langchain elements
llm = OpenAIChat()
memory = ConversationSummaryMemory(llm=llm)


# Define shared agent response
class AgentResponse(BaseModel):
    result: str


# Define root API
@app.get("/")
def read_root():
    return {"Hello": "World"}


# Setup conversation agent
conversation_agent_executor = conversational_agent.setup_agent(
    llm=llm,
    memory=memory)


# Define conversation agent API
class ConversationAgentRequest(BaseModel):
    content: str


@app.post("/agents/conversation", response_model=AgentResponse)
async def conversation_agent(request: ConversationAgentRequest):
    result = conversation_agent_executor.run(
        input=request.content,
        chat_history=memory.chat_memory)
    return {"result": result}
