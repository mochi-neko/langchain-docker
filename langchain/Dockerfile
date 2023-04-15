FROM --platform=arm64 python:3.9

RUN apt-get -y update

WORKDIR /app

RUN pip install poetry

COPY pyproject.toml poetry.lock ./

RUN poetry install --no-root

COPY . .

ENV PYTHONPATH="/app:${PYTHONPATH}"

EXPOSE 8000

ENTRYPOINT ["poetry", "run"]

CMD ["uvicorn", "src.main:app", "--reload", "--host", "0.0.0.0", "--port", "8000"]