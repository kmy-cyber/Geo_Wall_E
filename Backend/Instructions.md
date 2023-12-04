```
extends Node

func _ready():
    var http_request = HTTPRequest.new()  # Crear una nueva instancia de HTTPRequest
    self.add_child(http_request)  # Agregar HTTPRequest al árbol de escena

    # Conectar la señal "request_completed" a una función en este script
    http_request.connect("request_completed", self, "_on_request_completed")

    # Iniciar una solicitud GET
    var error = http_request.request("http://tu-api.com/ruta")
    if error == OK:
        print("Solicitud iniciada!")
    else:
        print("Error al iniciar la solicitud.")

func _on_request_completed(result, response_code, headers, body):
    var json = JSON.parse(body.get_string_from_utf8())
    print("Respuesta del servidor: ", json.result)

```