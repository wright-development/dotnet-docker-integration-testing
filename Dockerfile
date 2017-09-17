FROM microsoft/dotnet:1.1.2-sdk as builder
COPY . /code
WORKDIR /code/src/TodoService
RUN dotnet restore && dotnet publish -c Release -o publish

FROM microsoft/dotnet:1.1.2-runtime
COPY --from=builder /code/src/TodoService/publish /app
WORKDIR /app
ENV ASPNETCORE_URLS="http://*:5000"
EXPOSE 5000
RUN curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > wait_for_it.sh
ENTRYPOINT [ "dotnet", "/app/TodoService.dll" ]