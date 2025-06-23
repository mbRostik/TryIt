@echo off
echo Starting FastAPI apps...

REM Launch webApi.py on port 8000
cd /d D:\C\Projects\AI_TryIt\WebApi
start "webApi" cmd /k uvicorn webApi:app --host 127.0.0.1 --port 8000

REM Launch recommend_api.py on port 8001
cd /d D:\C\Projects\AI_TryIt\GNN\SecondGNN
start "recommend_api" cmd /k uvicorn recommend_api:app --host 127.0.0.1 --port 8001

REM Launch recommend_api.py on port 8001
cd /d D:\C\Projects\AI_TryIt\Message_Validation
start "predict_text" cmd /k uvicorn predict_text:app --host 127.0.0.1 --port 8004

REM Launch UpdateTextGraphAPI.py on port 8002
REM cd /d D:\C\Projects\AI_TryIt\GNN\SecondGNN
REM start "text_graph_api" cmd /k uvicorn UpdateTextGraphAPI:app --host 127.0.0.1 --port 8002

REM Launch UpdatePhotoGraphAPI.py on port 8003
REM cd /d D:\C\Projects\AI_TryIt\GNN\SecondGNN
REM start "photo_graph_api" cmd /k uvicorn UpdatePhotoGraphAPI:app --host 127.0.0.1 --port 8003

echo All APIs launched in separate terminals.
