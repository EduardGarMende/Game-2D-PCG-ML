import pandas as pd
import numpy as np
import os

def generar_datos_sinteticos(input_path, output_path, multiplicador=49, nivel_ruido=0.2):
    """
    Genera datos sintéticos aplicando ruido gaussiano basado en la desviación estándar 
    de cada clase para mantener la cohesión de los perfiles.
    """
    print(f"Leyendo dataset original desde: {input_path}")
    df = pd.read_csv(input_path)
    
    target_col = 'PerfilJugador'
    cat_cols = ['RewardChosen']
    num_cols = [c for c in df.columns if c not in [target_col] + cat_cols]
    
    # Calcular la desviación estándar de cada variable agrupada por el perfil del jugador
    std_por_perfil = df.groupby(target_col)[num_cols].std().fillna(0)
    
    filas_aumentadas = []
    
    for idx, row in df.iterrows():
        filas_aumentadas.append(row.to_dict())
        
        perfil_actual = row[target_col]
        
        # Generar N muestras sintéticas a partir de esta fila
        for _ in range(multiplicador):
            nueva_fila = row.copy()
            
            for col in num_cols:
                # Extraer la desviación estándar de esta columna específica para este perfil
                std_col = std_por_perfil.loc[perfil_actual, col]
                
                # Generar ruido gaussiano
                ruido = np.random.normal(0, std_col * nivel_ruido)
                nuevo_valor = nueva_fila[col] + ruido
                
                # Aplicar límites lógicos para que los datos tengan sentido
                if col in ['Precision', 'RangedRatio', 'TimeInRiskZone']:
                    nuevo_valor = max(0.0, min(1.0, nuevo_valor))
                else:
                    nuevo_valor = max(0.0, nuevo_valor)
                    
                nueva_fila[col] = round(nuevo_valor, 2)
                
            filas_aumentadas.append(nueva_fila.to_dict())
            
    # Crear el nuevo DataFrame
    df_aumentado = pd.DataFrame(filas_aumentadas)
    
    # Mezclar el dataset para que los perfiles no estén ordenados
    df_aumentado = df_aumentado.sample(frac=1, random_state=42).reset_index(drop=True)
    
    # Guardar el nuevo CSV
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    df_aumentado.to_csv(output_path, index=False)
    
    print("-" * 50)
    print(f"Data Augmentation completado con éxito.")
    print(f"Muestras originales: {len(df)}")
    print(f"Muestras totales generadas: {len(df_aumentado)}")
    print(f"Guardado en: {output_path}")
    print("-" * 50)

if __name__ == "__main__":
    
    ruta_entrada = "data/telemetria_entrenamiento.csv"
    ruta_salida = "data/augmented_data.csv"
    
    generar_datos_sinteticos(
        input_path=ruta_entrada, 
        output_path=ruta_salida, 
        multiplicador=49, 
        nivel_ruido=0.15 # El ruido es del 15% de la desviación estándar
    )